import React from "react";
import { Outlet, useLoaderData, useNavigate } from "react-router-dom";
import { Stack } from "@mui/material";

import { ListName } from "../../models";

import ListsPageToolbar from "./lists-page-toolbar";

export const listsPageLoader = async () => {
  const getRequest = new Request(`${process.env.PUBLIC_URL}/api/lists/names`, {
    method: "GET",
  });
  const response = await fetch(getRequest);
  if (response.status === 401) {
    return [] as ListName[];
  }
  const retval = await response.json();
  return retval;
};

const ListsPage = () => {
  const loaded = useLoaderData() as ListName[];
  const navigate = useNavigate();

  const handleSelectedListChanged = (listName: ListName) => {
    navigate(`/${listName.id}`);
  };

  return (
    <Stack spacing={4} sx={{ px: 2, py: 4 }}>
      <ListsPageToolbar
        listNames={loaded}
        onSelectedListNameChanged={handleSelectedListChanged}
      />

      <Outlet />
    </Stack>
  );
};

export default ListsPage;
