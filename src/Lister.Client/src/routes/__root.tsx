import React, { MouseEvent, useState } from "react";
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
import { AccountCircle } from "@mui/icons-material";
import {
  createRootRouteWithContext,
  Outlet,
  useRouter,
} from "@tanstack/react-router";
import { QueryClient } from "@tanstack/react-query";

import { SideDrawer } from "../components";

const RootComponent = () => {
  const router = useRouter();
  const navigate = Route.useNavigate();

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
      await router.invalidate();
      await navigate({ to: "/" });
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
  queryClient: QueryClient;
}>()({
  component: RootComponent,
});
