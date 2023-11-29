import React, { useState } from "react";
import {
  AppBar,
  Box,
  IconButton,
  Menu,
  MenuItem,
  Toolbar,
} from "@mui/material";
import { AccountCircle } from "@mui/icons-material";

import { useAuth } from "../../auth";

const TopSection = () => {
  const { signOut } = useAuth();
  const [anchorEl, setAnchorEl] = useState<HTMLButtonElement | null>(null);

  const handleOpenMenu = (element: HTMLButtonElement): void => {
    setAnchorEl(element);
  };

  const handleCloseMenu = () => {
    setAnchorEl(null);
  };

  return (
    <AppBar
      elevation={0}
      sx={(theme) => ({
        backgroundColor: theme.palette.background.paper,
        borderBottom: `1px solid ${theme.palette.divider}`,
      })}
    >
      <Toolbar>
        <Box sx={{ flexGrow: 1 }} />

        <IconButton
          color="default"
          onClick={(event) => handleOpenMenu(event.currentTarget)}
        >
          <AccountCircle />
        </IconButton>

        <Menu
          anchorEl={anchorEl}
          keepMounted
          open={Boolean(anchorEl)}
          onClose={handleCloseMenu}
        >
          <MenuItem onClick={() => signOut()}>Sign Out</MenuItem>
        </Menu>
      </Toolbar>
    </AppBar>
  );
};

export default TopSection;
