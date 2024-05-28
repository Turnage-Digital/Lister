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

import { Loading, SideDrawer, useAuth, useLoad } from "../components";

const Root = () => {
  const { logout } = useAuth();
  const { loading } = useLoad();

  const [userMenuAnchorElement, setUserMenuAnchorElement] =
    useState<HTMLButtonElement | null>(null);
  const handleOpenUserMenu = (event: MouseEvent<HTMLButtonElement>) => {
    setUserMenuAnchorElement(event.currentTarget);
  };
  const handleCloseUserMenu = () => {
    setUserMenuAnchorElement(null);
  };

  const content = loading ? <Loading /> : <Outlet />;

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
                <MenuItem onClick={logout}>Log Out</MenuItem>
              </Menu>
            </Box>
          </Toolbar>
        </AppBar>

        <Container maxWidth="xl">
          <Box sx={(theme) => ({ ...theme.mixins.toolbar })} />
          {content}
        </Container>
      </Stack>

      <SideDrawer />
    </>
  );
};

export default Root;
