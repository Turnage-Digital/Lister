import React from "react";
import { Dialog, Drawer, useMediaQuery, useTheme } from "@mui/material";

import { useSideDrawer } from "./side-drawer-provider";

const SideDrawer = () => {
  const { content, closeDrawer } = useSideDrawer();
  const theme = useTheme();
  const fullScreen = useMediaQuery(theme.breakpoints.up("sm"));

  // const outerContent = (
  //   <>
  //     <Box
  //       sx={(theme) => ({
  //         display: "flex",
  //         alignItems: "center",
  //         padding: theme.spacing(2),
  //         ...theme.mixins.toolbar,
  //       })}
  //     >
  //       <Typography variant="h6" sx={{ flexGrow: 1 }}>
  //         {title}
  //       </Typography>
  //
  //       <IconButton onClick={closeDrawer}>
  //         <Close />
  //       </IconButton>
  //     </Box>
  //     <Box sx={{ overflowY: "auto", flexGrow: 1 }}>{innerContent}</Box>
  //   </>
  // );

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
