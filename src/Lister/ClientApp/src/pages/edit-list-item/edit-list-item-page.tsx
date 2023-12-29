import React from "react";
import { ActionFunctionArgs, LoaderFunctionArgs } from "react-router-dom";

export const editListItemPageLoader = async ({
  params,
}: LoaderFunctionArgs) => {
  if (!params.listId) {
    return null;
  }

  const getRequest = new Request(
    `${process.env.PUBLIC_URL}/api/lists/${params.listId}`,
    {
      method: "GET",
    }
  );
  const response = await fetch(getRequest);
  if (response.status === 401) {
    return null;
  }
  if (response.status === 404) {
    return null;
  }
  const retval = await response.json();
  return retval;
};

export const editListItemPageAction = async ({
  params,
  request,
}: ActionFunctionArgs) => {
  if (params.listId) {
    //
  }
};

const EditListItemPage = () => {
  return <h1>Edit List Item</h1>;
};

export default EditListItemPage;
