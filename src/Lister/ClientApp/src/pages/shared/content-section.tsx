import React from "react";
import { Box } from "@mui/material";
import { Outlet } from "react-router-dom";

const ContentSection = () => {
  return (
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
  );
};

export default ContentSection;
