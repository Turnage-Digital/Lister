import React, { MouseEvent, useState } from "react";
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

import ListsPageToolbar from "./lists-page-toolbar";

export const listsPageLoader = async () => {
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

const ListsPage = () => {
  const loaded = useLoaderData() as List[];
  const [selectedList, setSelectedList] = useState<List | null>(loaded[0]);

  const content = selectedList ? (
    <Stack spacing={4} divider={<Divider />} sx={{ p: 4 }}>
      <ListsPageToolbar
        lists={loaded}
        selectedList={selectedList}
        onSelectedListChanged={setSelectedList}
      />
    </Stack>
  ) : (
    <></>
  );

  return <Container>{content}</Container>;
};

export default ListsPage;
