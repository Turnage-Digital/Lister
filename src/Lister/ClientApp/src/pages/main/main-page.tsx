import React from "react";
import { useLoaderData } from "react-router-dom";
import { Container } from "@mui/material";

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

  return <Container />;
};

export default MainPage;
