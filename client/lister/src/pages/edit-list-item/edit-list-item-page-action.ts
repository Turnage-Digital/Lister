import { ActionFunctionArgs, redirect } from "react-router-dom";

export const editListItemPageAction = async ({
  params,
  request,
}: ActionFunctionArgs) => {
  const data = await request.formData();
  const serialized = data.get("serialized") as string;

  const postRequest = new Request(`/api/lists/${params.listId}/items/create`, {
    headers: {
      "Content-Type": "application/json",
    },
    method: "POST",
    body: serialized,
  });

  await fetch(postRequest);
  return redirect(`/${params.listId}`);
};
