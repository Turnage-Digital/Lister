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
import React, { MouseEvent, useState } from "react";
import { Outlet } from "react-router-dom";

import {
  ListDefinitionProvider,
  Loading,
  SideDrawer,
  SignInDialog,
  useAuth,
} from "../components";

const Root = () => {
  const { loggedIn, logout } = useAuth();

  const [userMenuAnchorElement, setUserMenuAnchorElement] =
    useState<HTMLButtonElement | null>(null);

  const handleOpenUserMenu = (event: MouseEvent<HTMLButtonElement>) => {
    setUserMenuAnchorElement(event.currentTarget);
  };

  const handleCloseUserMenu = () => {
    setUserMenuAnchorElement(null);
  };

  const handleLogoutClick = async () => {
    setUserMenuAnchorElement(null);
    await logout();
  };

  return (
    <ListDefinitionProvider>
      <Stack
        sx={{
          minWidth: "100%",
          height: "100vh",
        }}
      >
        <AppBar>
          <Toolbar>
            <Box sx={{ flexGrow: 1 }} />
            {loggedIn && (
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
          {loggedIn && <Outlet />}
        </Container>
      </Stack>

      <SideDrawer />
      <SignInDialog />
      <Loading />
    </ListDefinitionProvider>
  );
};

export default Root;
