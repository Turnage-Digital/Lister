import React from "react";
import { Dialog, Drawer, useMediaQuery, useTheme } from "@mui/material";

import { useSideDrawer } from "./side-drawer-provider";

const SideDrawer = () => {
  const { content, closeDrawer } = useSideDrawer();
  const theme = useTheme();
  const fullScreen = useMediaQuery(theme.breakpoints.up("sm"));

  return fullScreen ? (
    <Drawer
      anchor="right"
      open={content !== null}
      onClose={closeDrawer}
      PaperProps={{
        sx: { width: 500 },
      }}
    >
      {content}
    </Drawer>
  ) : (
    <Dialog open={content !== null} onClose={closeDrawer} fullScreen>
      {content}
    </Dialog>
  );
};

export default SideDrawer;
