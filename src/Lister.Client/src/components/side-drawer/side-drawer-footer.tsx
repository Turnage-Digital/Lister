import * as React from "react";
import { PropsWithChildren } from "react";

import { Box } from "@mui/material";

type Props = PropsWithChildren;

const SideDrawerFooter = ({ children }: Props) => {
  return (
    <Box
      sx={(theme) => ({
        padding: 2,
        borderTop: 1,
        borderColor: theme.palette.divider,
        backgroundColor: theme.palette.grey[50],
        display: "flex",
        width: "100%",
        zIndex: 9999
      })}
    >
      {children}
    </Box>
  );
};

export default SideDrawerFooter;
