import React, { useState } from "react";
import { useLoaderData } from "react-router-dom";
import {
  Box,
  Button,
  Container,
  Divider,
  Hidden,
  IconButton,
  ListItemIcon,
  ListItemText,
  Menu,
  MenuItem,
  Stack,
  Typography,
} from "@mui/material";
import Grid from "@mui/material/Unstable_Grid2";
import { AddCircle, ExpandCircleDown, PlaylistAdd } from "@mui/icons-material";

import { List } from "../../models";

export const dashboardPageLoader = async () => {
  const getRequest = new Request(`${process.env.PUBLIC_URL}/api/lists/names`, {
    method: "GET",
  });
  const response = await fetch(getRequest);
  if (response.status === 401) {
    return [] as List[];
  }
  const retval = await response.json();
  return retval;
};

const DashboardPage = () => {
  const loaded = useLoaderData() as List[];

  const [selectedIndex, setSelectedIndex] = useState<number>(0);
  const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);
  const open = Boolean(anchorEl);

  const handleButtonClick = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleMenuItemClick = (index: number) => {
    setSelectedIndex(index);
    setAnchorEl(null);
  };

  const handleMenuClose = () => {
    setAnchorEl(null);
  };

  return (
    <Container>
      <Stack spacing={4} divider={<Divider />} sx={{ p: 4 }}>
        <Box alignItems="center" sx={{ display: "flex" }}>
          <Box sx={{ flexGrow: 1 }}>
            <Grid container direction="row" alignItems="center" spacing={2}>
              <Grid>
                <Typography variant="h4" component="h1">
                  {loaded[selectedIndex].name}
                </Typography>
              </Grid>
              <Grid>
                <IconButton
                  color="primary"
                  size="large"
                  onClick={handleButtonClick}
                  sx={{ ml: 2 }}
                >
                  <ExpandCircleDown />
                </IconButton>
              </Grid>
            </Grid>

            <Menu anchorEl={anchorEl} open={open} onClose={handleMenuClose}>
              {loaded.map((list, index) => (
                <MenuItem
                  key={list.id}
                  value={index}
                  selected={index === selectedIndex}
                  onClick={() => handleMenuItemClick(index)}
                >
                  {list.name}
                </MenuItem>
              ))}
              <Divider />
              <MenuItem>
                <ListItemIcon>
                  <PlaylistAdd fontSize="small" />
                </ListItemIcon>
                <ListItemText>Create a List</ListItemText>
              </MenuItem>
            </Menu>
          </Box>

          <Hidden mdDown>
            <Box
              sx={{
                flexGrow: 2,
                display: "flex",
                justifyContent: "flex-end",
              }}
            >
              <Button variant="contained" startIcon={<AddCircle />}>
                Create an Item
              </Button>
            </Box>
          </Hidden>
        </Box>
      </Stack>
    </Container>
  );
};

export default DashboardPage;
