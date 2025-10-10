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
import { NotificationsBell, SideDrawer, UserMenu } from "./components";
import { connectChangeFeed, createChangeFeedRouter } from "./lib/sse";

const Shell = () => {
  const auth = useAuth();
  const queryClient = useQueryClient();
  const theme = useTheme();
  const location = useLocation();

  React.useEffect(() => {
    if (auth.status !== "loggedIn") {
      return undefined;
    }

    const handler = createChangeFeedRouter({
      "Lister.Core.Domain.IntegrationEvents.ListItemCreatedIntegrationEvent":
        () => {
          queryClient.invalidateQueries({
            queryKey: ["list-items"],
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
        },
      "Lister.Core.Domain.IntegrationEvents.ListDeletedIntegrationEvent":
        () => {
          queryClient.invalidateQueries({ queryKey: ["list-names"] });
          queryClient.invalidateQueries({
            queryKey: ["list-items"],
            exact: false,
          });
        },
      "Lister.Core.Domain.IntegrationEvents.ListUpdatedIntegrationEvent":
        () => {
          queryClient.invalidateQueries({
            queryKey: ["list-definition"],
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

      <SideDrawer />
    </>
  );
};

export default Shell;
