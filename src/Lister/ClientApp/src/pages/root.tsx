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
  LoaderFunctionArgs,
  Outlet,
  redirect,
  useFetcher,
} from "react-router-dom";

import { SideDrawer } from "../components";

export const rootLoader = async ({ request }: LoaderFunctionArgs) => {
  const postRequest = new Request(
    `${process.env.PUBLIC_URL}/api/users/claims`,
    {
      method: "GET",
    }
  );
  const response = await fetch(postRequest);
  if (response.status === 401) {
    const params = new URLSearchParams();
    params.set("callbackUrl", new URL(request.url).pathname);
    return redirect(`/sign-in?${params.toString()}`);
  }
  const retval = await response.json();
  return retval;
};

const Root = () => {
  const fetcher = useFetcher();

  const [userMenuAnchorElement, setUserMenuAnchorElement] =
    useState<HTMLButtonElement | null>(null);
  const handleOpenUserMenu = (event: MouseEvent<HTMLButtonElement>) => {
    setUserMenuAnchorElement(event.currentTarget);
  };
  const handleCloseUserMenu = () => {
    setUserMenuAnchorElement(null);
  };

  const handleLogoutClick = async () => {
    fetcher.submit({}, { method: "POST", action: "/sign-out" });
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
                <MenuItem onClick={handleLogoutClick}>Sign Out</MenuItem>
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

export default Root;
