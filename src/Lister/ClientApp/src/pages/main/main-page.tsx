import React, { useState } from "react";
import { useLoaderData } from "react-router-dom";
import {
  Box,
  Button,
  Container,
  Divider,
  FormControl,
  Hidden,
  MenuItem,
  Select,
  Stack,
} from "@mui/material";
import { Add } from "@mui/icons-material";

import { List } from "../../models";

export const mainPageLoader = async () => {
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

const MainPage = () => {
  const loaded = useLoaderData() as List[];
  const [selectedIndex, setSelectedIndex] = useState<number>(0);

  const handleMenuItemClick = (index: number) => {
    setSelectedIndex(index);
  };

  return (
    <Container>
      <Stack spacing={4} divider={<Divider />} sx={{ p: 4 }}>
        <Box alignItems="center" sx={{ display: "flex" }}>
          <Box sx={{ flexGrow: 1 }}>
            <FormControl
              fullWidth
              sx={{
                background: "white",
              }}
            >
              <Select value={selectedIndex}>
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
              </Select>
            </FormControl>
          </Box>

          <Hidden mdDown>
            <Box
              sx={{
                flexGrow: 2,
                display: "flex",
                justifyContent: "flex-end",
              }}
            >
              <Button variant="contained" size="small" startIcon={<Add />}>
                Item
              </Button>
            </Box>
          </Hidden>
        </Box>
      </Stack>
    </Container>
  );
};

export default MainPage;
