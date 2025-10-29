import * as React from "react";

import {
  AppBar,
  Box,
  CircularProgress,
  Container,
  Toolbar,
  Typography,
  useTheme,
} from "@mui/material";
import { useQueryClient } from "@tanstack/react-query";
import { Navigate, Outlet, useLocation } from "react-router-dom";

import { useAuth } from "./auth";
import { NotificationsBell, UserMenu } from "./components";
import { connectChangeFeed, createChangeFeedRouter } from "./lib/sse";

import type { MigrationProgressRecord } from "./models";

const Shell = () => {
  const auth = useAuth();
  const queryClient = useQueryClient();
  const theme = useTheme();
  const location = useLocation();

  React.useEffect(() => {
    if (auth.status !== "loggedIn") {
      return undefined;
    }

    const readValue = (data: Record<string, unknown>, key: string) =>
      data?.[key] ?? data?.[key.charAt(0).toLowerCase() + key.slice(1)];

    const readString = (data: Record<string, unknown>, key: string) => {
      const value = readValue(data, key);
      return typeof value === "string" && value.length > 0 ? value : undefined;
    };

    const readNumber = (data: Record<string, unknown>, key: string) => {
      const value = readValue(data, key);
      return typeof value === "number" ? value : undefined;
    };

    const toProgressKey = (listId: string, correlationId: string) =>
      ["list-migration-progress", listId, correlationId] as const;

    const updateMigrationProgress = (
      listId: string | undefined,
      correlationId: string | undefined,
      updater: (current: MigrationProgressRecord) => MigrationProgressRecord,
    ) => {
      if (!listId || !correlationId) {
        return;
      }

      queryClient.setQueryData<MigrationProgressRecord | null>(
        toProgressKey(listId, correlationId),
        (previous) => {
          const base: MigrationProgressRecord = previous ?? {
            listId,
            correlationId,
            stage: "Pending",
            createdOn: new Date().toISOString(),
            updatedAt: new Date().toISOString(),
          };
          return updater(base);
        },
      );
    };

    const handler = createChangeFeedRouter({
      "Lister.Core.Domain.IntegrationEvents.ListItemCreatedIntegrationEvent":
        () => {
          queryClient.invalidateQueries({
            queryKey: ["list-items"],
            exact: false,
          });
          queryClient.invalidateQueries({
            queryKey: ["item-history"],
            exact: false,
          });
          queryClient.invalidateQueries({
            queryKey: ["list-history"],
            exact: false,
          });
        },
      "Lister.Core.Domain.IntegrationEvents.ListItemUpdatedIntegrationEvent":
        () => {
          queryClient.invalidateQueries({
            queryKey: ["list-items"],
            exact: false,
          });
          queryClient.invalidateQueries({
            queryKey: ["list-item"],
            exact: false,
          });
          queryClient.invalidateQueries({
            queryKey: ["item-history"],
            exact: false,
          });
        },
      "Lister.Core.Domain.IntegrationEvents.ListItemDeletedIntegrationEvent":
        () => {
          queryClient.invalidateQueries({
            queryKey: ["list-items"],
            exact: false,
          });
          queryClient.invalidateQueries({
            queryKey: ["list-item"],
            exact: false,
          });
          queryClient.invalidateQueries({
            queryKey: ["item-history"],
            exact: false,
          });
        },
      "Lister.Core.Domain.IntegrationEvents.ListDeletedIntegrationEvent":
        () => {
          queryClient.invalidateQueries({ queryKey: ["list-names"] });
          queryClient.invalidateQueries({
            queryKey: ["list-items"],
            exact: false,
          });
          queryClient.invalidateQueries({
            queryKey: ["list-history"],
            exact: false,
          });
        },
      "Lister.Core.Domain.IntegrationEvents.ListUpdatedIntegrationEvent":
        () => {
          queryClient.invalidateQueries({
            queryKey: ["list-definition"],
            exact: false,
          });
          queryClient.invalidateQueries({
            queryKey: ["list-history"],
            exact: false,
          });
        },
      "Lister.Notifications.Domain.Events.NotificationCreatedEvent": () => {
        queryClient.invalidateQueries({
          queryKey: ["notifications"],
          exact: false,
        });
        queryClient.invalidateQueries({
          queryKey: ["notifications-unread-count"],
          exact: false,
        });
      },
      "Lister.Notifications.Domain.Events.NotificationProcessedEvent": () => {
        queryClient.invalidateQueries({
          queryKey: ["notifications"],
          exact: false,
        });
      },
      "Lister.Notifications.Domain.Events.NotificationReadEvent": () => {
        queryClient.invalidateQueries({
          queryKey: ["notifications"],
          exact: false,
        });
        queryClient.invalidateQueries({
          queryKey: ["notifications-unread-count"],
          exact: false,
        });
      },
      "Lister.Notifications.Domain.Events.AllNotificationsReadEvent": () => {
        queryClient.invalidateQueries({
          queryKey: ["notifications"],
          exact: false,
        });
        queryClient.invalidateQueries({
          queryKey: ["notifications-unread-count"],
          exact: false,
        });
      },
      "Lister.Notifications.Domain.Events.NotificationDeliveryAttemptedEvent":
        () => {
          queryClient.invalidateQueries({
            queryKey: ["notification"],
            exact: false,
          });
        },
      "Lister.Notifications.Domain.Events.NotificationRuleCreatedEvent": () => {
        queryClient.invalidateQueries({
          queryKey: ["notifications"],
          exact: false,
        });
      },
      "Lister.Notifications.Domain.Events.NotificationRuleUpdatedEvent": () => {
        queryClient.invalidateQueries({
          queryKey: ["notifications"],
          exact: false,
        });
      },
      "Lister.Notifications.Domain.Events.NotificationRuleDeletedEvent": () => {
        queryClient.invalidateQueries({
          queryKey: ["notifications"],
          exact: false,
        });
      },
      "Lister.Core.Domain.IntegrationEvents.ListMigrationRequestedIntegrationEvent":
        (data) => {
          const payload = data as Record<string, unknown>;
          const listId = readString(payload, "ListId");
          const correlationId = readString(payload, "CorrelationId");
          const requestedBy = readString(payload, "RequestedBy");

          updateMigrationProgress(listId, correlationId, (current) => ({
            ...current,
            stage: "Pending",
            requestedBy: requestedBy ?? current.requestedBy,
            lastMessage: "Migration queued.",
            percent: 0,
            updatedAt: new Date().toISOString(),
          }));
        },
      "Lister.Core.Domain.IntegrationEvents.ListMigrationProgressIntegrationEvent":
        (data) => {
          const payload = data as Record<string, unknown>;
          const listId = readString(payload, "ListId");
          const correlationId = readString(payload, "CorrelationId");
          const message =
            readString(payload, "Message") ?? "Migration in progress.";
          const percent = readNumber(payload, "Percent");
          const occurredOn =
            readString(payload, "OccurredOn") ?? new Date().toISOString();

          const inferredStage = message.includes("Backup list removed")
            ? "Archived"
            : "Running";

          updateMigrationProgress(listId, correlationId, (current) => ({
            ...current,
            stage: inferredStage,
            lastMessage: message,
            percent:
              inferredStage === "Running" && typeof percent === "number"
                ? percent
                : current.percent,
            updatedAt: occurredOn,
          }));
        },
      "Lister.Core.Domain.IntegrationEvents.ListMigrationCompletedIntegrationEvent":
        (data) => {
          const payload = data as Record<string, unknown>;
          const listId = readString(payload, "ListId");
          const correlationId = readString(payload, "CorrelationId");
          const occurredOn =
            readString(payload, "OccurredOn") ?? new Date().toISOString();
          const itemsProcessed = readNumber(payload, "ItemsProcessed");

          updateMigrationProgress(listId, correlationId, (current) => ({
            ...current,
            stage: "Completed",
            lastMessage:
              typeof itemsProcessed === "number"
                ? `Migration completed — ${itemsProcessed} item${itemsProcessed === 1 ? "" : "s"} processed.`
                : (current.lastMessage ?? "Migration completed."),
            itemsProcessed: itemsProcessed ?? current.itemsProcessed,
            percent: 100,
            updatedAt: occurredOn,
          }));

          if (listId) {
            queryClient.invalidateQueries({
              queryKey: ["list-definition", listId],
            });
            queryClient.invalidateQueries({
              queryKey: ["list-items"],
              exact: false,
            });
            queryClient.invalidateQueries({
              queryKey: ["list-history"],
              exact: false,
            });
          }
        },
      "Lister.Core.Domain.IntegrationEvents.ListMigrationFailedIntegrationEvent":
        (data) => {
          const payload = data as Record<string, unknown>;
          const listId = readString(payload, "ListId");
          const correlationId = readString(payload, "CorrelationId");
          const message = readString(payload, "Message") ?? "Migration failed.";
          const occurredOn =
            readString(payload, "OccurredOn") ?? new Date().toISOString();

          updateMigrationProgress(listId, correlationId, (current) => ({
            ...current,
            stage: "Failed",
            lastError: message,
            lastMessage: message,
            percent: 0,
            updatedAt: occurredOn,
          }));
        },
    });

    const { close } = connectChangeFeed(handler, {
      withCredentials: true,
      onError: () => {
        // Optionally add a toast or log
      },
    });

    return () => close();
  }, [auth.status, queryClient]);

  if (auth.status === "checking") {
    return (
      <Box
        sx={{
          minHeight: "100vh",
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
        }}
      >
        <CircularProgress />
      </Box>
    );
  }

  if (auth.status !== "loggedIn") {
    const callbackUrl = `${location.pathname}${location.search}${location.hash}`;
    const search = callbackUrl
      ? `?callbackUrl=${encodeURIComponent(callbackUrl)}`
      : "";
    return <Navigate to={`/sign-in${search}`} replace />;
  }

  return (
    <>
      <Box sx={{ minHeight: "100vh" }}>
        <AppBar
          position="fixed"
          sx={{
            backgroundColor: theme.palette.background.paper,
            color: theme.palette.text.primary,
            boxShadow: theme.shadows[1],
            borderBottom: `1px solid ${theme.palette.divider}`,
          }}
        >
          <Toolbar>
            <Typography
              component="h1"
              variant="h4"
              fontWeight="bold"
              color="primary"
              sx={{ flexGrow: 1 }}
            >
              Lister
            </Typography>

            <NotificationsBell />
            <UserMenu />
          </Toolbar>
        </AppBar>

        <Container
          component="main"
          maxWidth="xl"
          sx={{
            minHeight: "100vh",
            backgroundColor: theme.palette.background.default,
          }}
        >
          <Box sx={(muiTheme) => ({ ...muiTheme.mixins.toolbar })} />
          <Outlet />
        </Container>
      </Box>
    </>
  );
};

export default Shell;
