import * as React from "react";
import { PropsWithChildren } from "react";

import { Box } from "@mui/material";

type Props = PropsWithChildren;

const SideDrawerContent = ({ children }: Props) => {
  return (
    <Box sx={{ overflowY: "auto", display: "flex", flex: 1 }}>{children}</Box>
  );
};

export default SideDrawerContent;
