import React, { useState, MouseEvent } from "react";
import {
  AppBar,
  Box,
  IconButton,
  Menu,
  MenuItem,
  Toolbar,
} from "@mui/material";
import {
  LoaderFunctionArgs,
  Outlet,
  redirect,
  useFetcher,
} from "react-router-dom";
import { AccountCircle } from "@mui/icons-material";

export const layoutLoader = async ({ request }: LoaderFunctionArgs) => {
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

const Layout = () => {
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
    <Box sx={{ display: "flex" }}>
      <AppBar
        elevation={0}
        sx={(theme) => ({
          backgroundColor: theme.palette.background.paper,
          borderBottom: `1px solid ${theme.palette.divider}`,
        })}
      >
        <Toolbar>
          <Box sx={{ flexGrow: 1 }} />
          <Box sx={{ flexGrow: 0 }}>
            <IconButton color="primary" onClick={handleOpenUserMenu}>
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

      <Box
        component="main"
        sx={(theme) => ({
          background: theme.palette.background.default,
          flexGrow: 1,
        })}
      >
        <Box sx={(theme) => ({ ...theme.mixins.toolbar })} />
        <Outlet />
      </Box>
    </Box>
  );
};

export default Layout;
