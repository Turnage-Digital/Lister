import React from "react";
import { Box, IconButton, Typography } from "@mui/material";
import { Close } from "@mui/icons-material";

interface Props {
  title: string;
  onClose: () => void;
}

const SideDrawerHeader = ({ title, onClose }: Props) => {
  return (
    <Box
      sx={(theme) => ({
        display: "flex",
        alignItems: "center",
        padding: theme.spacing(2),
        ...theme.mixins.toolbar,
      })}
    >
      <Typography variant="h6" sx={{ flexGrow: 1 }}>
        {title}
      </Typography>

      <IconButton onClick={onClose}>
        <Close />
      </IconButton>
    </Box>
  );
};

export default SideDrawerHeader;
