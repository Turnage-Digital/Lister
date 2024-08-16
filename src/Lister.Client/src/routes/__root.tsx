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
import { createRootRoute, Outlet } from "@tanstack/react-router";

import { SideDrawer } from "../components";

const Root = () => {
  const [userMenuAnchorElement, setUserMenuAnchorElement] =
    useState<HTMLButtonElement | null>(null);

  const handleOpenUserMenu = (event: MouseEvent<HTMLButtonElement>) => {
    setUserMenuAnchorElement(event.currentTarget);
  };

  const handleCloseUserMenu = () => {
    setUserMenuAnchorElement(null);
  };

  const handleLogoutClick = async () => {};

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

// export const rootLoader = async ({ request }: LoaderFunctionArgs) => {
//   const postRequest = new Request("/identity/manage/info", {
//     method: "GET",
//   });
//   const response = await fetch(postRequest);
//   if (response.status === 401) {
//     const params = new URLSearchParams();
//     params.set("callbackUrl", new URL(request.url).pathname);
//     return redirect(`/sign-in?${params.toString()}`);
//   }
//   const retval = await response.json();
//   return retval;
// };

export const Route = createRootRoute({
  component: Root,
});
