import React from "react";
import { useLoaderData } from "react-router-dom";

import { IListDefsApi, ListDef, ListDefsApi } from "../../api";

const listDefsApi: IListDefsApi = new ListDefsApi(
  `${process.env.PUBLIC_URL}/api/list-defs`
);

export const listsPageLoader = async () => {
  const retval = await listDefsApi.get();
  return retval;
};

const ListsPage = () => {
  const loaded = useLoaderData() as ListDef[];

  return <div>Lists Page</div>;
};

export default ListsPage;
