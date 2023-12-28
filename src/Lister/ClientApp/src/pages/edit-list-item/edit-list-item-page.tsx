import React from "react";
import { ActionFunctionArgs, LoaderFunctionArgs } from "react-router-dom";

export const editListItemPageLoader = async ({
  params,
}: LoaderFunctionArgs) => {
  if (params.listId) {
    //
  }
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
