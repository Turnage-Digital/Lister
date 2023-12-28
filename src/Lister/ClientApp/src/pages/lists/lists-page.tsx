import React, { useEffect } from "react";
import {
  Outlet,
  useLoaderData,
  useNavigate,
  useParams,
} from "react-router-dom";
import { Container, Stack } from "@mui/material";

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
  const params = useParams();
  const navigate = useNavigate();

  const loaded = useLoaderData() as List[];
  const selectedList = loaded.find((list) => list.id === params.listId);

  useEffect(() => {
    if (!selectedList && loaded.length > 0) {
      navigate(`/${loaded[0].id}`);
    }
  }, [loaded, navigate, selectedList]);

  const handleSelectedListChanged = (list: List) => {
    navigate(`/${list.id}`);
  };

  const content = selectedList ? (
    <Stack spacing={4} sx={{ p: 4 }}>
      <ListsPageToolbar
        lists={loaded}
        selectedList={selectedList}
        onSelectedListChanged={handleSelectedListChanged}
      />

      <Outlet />
    </Stack>
  ) : (
    <></>
  );

  return <Container>{content}</Container>;
};

export default ListsPage;
