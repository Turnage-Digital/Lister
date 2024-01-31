import React, { useEffect } from "react";
import {
  Outlet,
  useLoaderData,
  useNavigate,
  useParams,
} from "react-router-dom";
import { Container, Stack } from "@mui/material";

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
  const params = useParams();
  const navigate = useNavigate();

  const selectedListName = loaded.find((list) => list.id === params.listId);

  useEffect(() => {
    if (!selectedListName && loaded.length > 0) {
      navigate(`/${loaded[0].id}`);
    }
  }, [loaded, navigate, selectedListName]);

  const handleSelectedListChanged = (listName: ListName) => {
    navigate(`/${listName.id}`);
  };

  const content = selectedListName ? (
    <Stack spacing={4} sx={{ px: 2, py: 4 }}>
      <ListsPageToolbar
        listNames={loaded}
        selectedListName={selectedListName}
        onSelectedListNameChanged={handleSelectedListChanged}
      />

      <Outlet />
    </Stack>
  ) : (
    <></>
  );

  return <>{content}</>;
};

export default ListsPage;
