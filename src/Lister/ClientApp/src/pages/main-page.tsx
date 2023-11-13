import React from "react";
import { useLoaderData } from "react-router-dom";
import { Container } from "@mui/material";

import { IListsApi, List, ListsApi } from "../api";

const listsApi: IListsApi = new ListsApi(`${process.env.PUBLIC_URL}/api/lists`);

export const mainPageLoader = async () => {
  const retval = await listsApi.getNames();
  return retval;
};

const MainPage = () => {
  const loaded = useLoaderData() as List[];

  return <Container />;
};

export default MainPage;
