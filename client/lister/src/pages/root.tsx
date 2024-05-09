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
import { Outlet } from "react-router-dom";

import { SideDrawer, useAuth } from "../components";

const Root = () => {
  const { signOut } = useAuth();

  const [userMenuAnchorElement, setUserMenuAnchorElement] =
    useState<HTMLButtonElement | null>(null);
  const handleOpenUserMenu = (event: MouseEvent<HTMLButtonElement>) => {
    setUserMenuAnchorElement(event.currentTarget);
  };
  const handleCloseUserMenu = () => {
    setUserMenuAnchorElement(null);
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
                <MenuItem onClick={signOut}>Sign Out</MenuItem>
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
