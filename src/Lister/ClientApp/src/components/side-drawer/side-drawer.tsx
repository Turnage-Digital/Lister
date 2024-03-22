import React, { PropsWithChildren } from "react";
import { Dialog, Drawer, useMediaQuery, useTheme } from "@mui/material";

type Props = PropsWithChildren<{
  open: boolean;
  onClose: () => void;
}>;

const SideDrawer = ({ open, onClose, children }: Props) => {
  const theme = useTheme();
  const fullScreen = useMediaQuery(theme.breakpoints.up("sm"));

  return fullScreen ? (
    <Drawer
      anchor="right"
      open={open}
      onClose={onClose}
      PaperProps={{
        sx: { width: 500 },
      }}
    >
      {children}
    </Drawer>
  ) : (
    <Dialog open={open} onClose={onClose} fullScreen>
      {children}
    </Dialog>
  );
};

export default SideDrawer;
