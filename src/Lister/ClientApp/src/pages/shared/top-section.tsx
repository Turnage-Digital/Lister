import React from "react";
import { AppBar, Box, Button, Toolbar } from "@mui/material";
import { useFetcher } from "react-router-dom";

const TopSection = () => {
  const fetcher = useFetcher();

  return (
    <AppBar
      elevation={0}
      sx={(theme) => ({
        backgroundColor: theme.palette.background.paper,
        borderBottom: `1px solid ${theme.palette.divider}`,
      })}
    >
      <Toolbar>
        <Box sx={{ flexGrow: 1 }} />

        <fetcher.Form method="post" action="/sign-out">
          <Button type="submit">Sign Out</Button>
        </fetcher.Form>
      </Toolbar>
    </AppBar>
  );
};

export default TopSection;
