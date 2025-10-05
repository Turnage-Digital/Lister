import * as React from "react";

import {
  AppBar,
  Box,
  Container,
  Toolbar,
  Typography,
  useTheme,
} from "@mui/material";
import { QueryClient } from "@tanstack/react-query";
import { createRootRouteWithContext, Outlet } from "@tanstack/react-router";

import { Auth } from "../auth";
import { SideDrawer, UserMenu, NotificationsBell } from "../components";
import { connectChangeFeed, createChangeFeedRouter } from "../lib/sse";

const RootComponent = () => {
  const { auth } = Route.useRouteContext({
    select: ({ auth }) => ({ auth }),
  });
  const { queryClient } = Route.useRouteContext({
    select: ({ queryClient }) => ({ queryClient }),
  });
  const theme = useTheme();

  React.useEffect(() => {
    if (auth.status !== "loggedIn") return;
    // SSE → Query cache routing
    const handler = createChangeFeedRouter({
      // Lists
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
      // Notifications
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

  if (auth.status !== "loggedIn") {
    return <Outlet />;
  }

  return (
    <>
      <Box sx={{ minHeight: "100vh" }}>
        {/* AppBar */}
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
            <UserMenu auth={auth} />
          </Toolbar>
        </AppBar>

        {/* Main content */}
        <Container
          component="main"
          maxWidth="xl"
          sx={{
            minHeight: "100vh",
            backgroundColor: theme.palette.background.default,
          }}
        >
          <Box sx={(theme) => ({ ...theme.mixins.toolbar })} />
          <Outlet />
        </Container>
      </Box>

      <SideDrawer />
    </>
  );
};

export const Route = createRootRouteWithContext<{
  auth: Auth;
  queryClient: QueryClient;
}>()({
  component: RootComponent,
});
