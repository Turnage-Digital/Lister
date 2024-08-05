import { Box } from "@mui/material";
import React, { PropsWithChildren } from "react";

type Props = PropsWithChildren;

const SideDrawerContent = ({ children }: Props) => {
  return (
    <Box sx={{ overflowY: "auto", display: "flex", flex: 1 }}>{children}</Box>
  );
};

export default SideDrawerContent;
