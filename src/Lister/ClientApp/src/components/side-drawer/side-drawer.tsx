import React from "react";
import {
  Box,
  Dialog,
  Drawer,
  IconButton,
  Typography,
  useMediaQuery,
  useTheme,
} from "@mui/material";
import { Close } from "@mui/icons-material";

import { useSideDrawer } from "./side-drawer-provider";

const SideDrawer = () => {
  const { title, content: innerContent, closeDrawer } = useSideDrawer();
  const theme = useTheme();
  const fullScreen = useMediaQuery(theme.breakpoints.up("sm"));

  const outerContent = (
    <>
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

        <IconButton onClick={closeDrawer}>
          <Close />
        </IconButton>
      </Box>
      <Box sx={{ overflowY: "auto", flexGrow: 1 }}>{innerContent}</Box>
    </>
  );

  return fullScreen ? (
    <Drawer
      anchor="right"
      open={innerContent !== null}
      onClose={closeDrawer}
      PaperProps={{
        sx: { width: 500 },
      }}
    >
      {outerContent}
    </Drawer>
  ) : (
    <Dialog open={innerContent !== null} onClose={closeDrawer} fullScreen>
      {outerContent}
    </Dialog>
  );
};

export default SideDrawer;
