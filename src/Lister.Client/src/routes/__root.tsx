import { AccountCircle } from "@mui/icons-material";
import {
  AppBar,
  Box,
  Container,
  IconButton,
  Menu,
  MenuItem,
  Stack,
  Toolbar,
} from "@mui/material";
import { QueryClient } from "@tanstack/react-query";
import {
  createRootRouteWithContext,
  Outlet,
  useRouter,
} from "@tanstack/react-router";
import React, { MouseEvent, useState } from "react";

import { Auth } from "../auth";
import { SideDrawer } from "../components";

const RootComponent = () => {
  const router = useRouter();
  const { auth, status } = Route.useRouteContext({
    select: ({ auth }) => ({ auth, status: auth.status }),
  });

  const [userMenuAnchorElement, setUserMenuAnchorElement] =
    useState<HTMLButtonElement | null>(null);

  const handleOpenUserMenu = (event: MouseEvent<HTMLButtonElement>) => {
    setUserMenuAnchorElement(event.currentTarget);
  };

  const handleCloseUserMenu = () => {
    setUserMenuAnchorElement(null);
  };

  const handleLogoutClick = async () => {
    const request = new Request("/identity/logout", {
      method: "POST",
    });
    const response = await fetch(request);
    if (response.ok) {
      auth.logout();
      router.invalidate();
    } else {
      // console.error("Failed to log out");
    }
  };

  return (
    <>
      <Stack
        sx={{
          minWidth: "100%",
          height: "100vh",
        }}
      >
        <AppBar>
          <Toolbar>
            <Box sx={{ flexGrow: 1 }} />

            {status === "loggedIn" && (
              <Box sx={{ flexGrow: 0 }}>
                <IconButton color="inherit" onClick={handleOpenUserMenu}>
                  <AccountCircle />
                </IconButton>
                <Menu
                  anchorEl={userMenuAnchorElement}
                  open={Boolean(userMenuAnchorElement)}
                  onClose={handleCloseUserMenu}
                >
                  <MenuItem onClick={handleLogoutClick}>Log Out</MenuItem>
                </Menu>
              </Box>
            )}
          </Toolbar>
        </AppBar>

        <Container maxWidth="xl">
          <Box sx={(theme) => ({ ...theme.mixins.toolbar })} />
          <Outlet />
        </Container>
      </Stack>

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
