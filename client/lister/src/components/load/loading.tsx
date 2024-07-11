import { Box, CircularProgress } from "@mui/material";
import React from "react";

import useLoad from "./use-load";

const Loading = () => {
  const { loading } = useLoad();

  return loading ? (
    <Box
      sx={{
        position: "fixed",
        top: 0,
        left: 0,
        right: 0,
        bottom: 0,
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        zIndex: 9999,
      }}
    >
      <CircularProgress />
    </Box>
  ) : null;
};

export default Loading;
