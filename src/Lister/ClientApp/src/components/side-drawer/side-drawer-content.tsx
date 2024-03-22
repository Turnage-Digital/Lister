import React, { PropsWithChildren } from "react";
import { Box } from "@mui/material";

type Props = PropsWithChildren;

const SideDrawerContent = ({ children }: Props) => (
  <Box sx={{ overflowY: "auto", flexGrow: 1 }}>{children}</Box>
);

export default SideDrawerContent;
