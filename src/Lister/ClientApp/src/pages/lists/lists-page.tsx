import React from "react";
import { useLoaderData } from "react-router-dom";

import { IListsApi, List, ListsApi } from "../../api";

const listsApi: IListsApi = new ListsApi(`${process.env.PUBLIC_URL}/api/lists`);

export const listsPageLoader = async () => {
  const retval = await listsApi.get();
  return retval;
};

const ListsPage = () => {
  const loaded = useLoaderData() as List[];

  return <div>Lists Page</div>;
};

export default ListsPage;
