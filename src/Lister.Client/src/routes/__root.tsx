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
import { SideDrawer, UserMenu } from "../components";

const RootComponent = () => {
  const { auth } = Route.useRouteContext({
    select: ({ auth }) => ({ auth }),
  });
  const theme = useTheme();

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
