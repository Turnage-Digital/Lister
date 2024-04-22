import React, { PropsWithChildren } from "react";
import { Box } from "@mui/material";

type Props = PropsWithChildren;

const SideDrawerFooter = ({ children }: Props) => {
  return (
    <Box
      sx={(theme) => ({
        position: "fixed",
        bottom: 0,
        width: 500,
        padding: 2,
        borderTop: 1,
        borderColor: theme.palette.divider,
        backgroundColor: theme.palette.grey[50],
        display: "flex",
        zIndex: 9999,
      })}
    >
      {children}
    </Box>
  );
};

export default SideDrawerFooter;
