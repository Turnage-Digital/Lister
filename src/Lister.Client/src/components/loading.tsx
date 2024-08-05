import { Box, CircularProgress } from "@mui/material";
import { useIsFetching } from "@tanstack/react-query";
import React from "react";

const Loading = () => {
  const isFetching = useIsFetching();

  return isFetching ? (
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
