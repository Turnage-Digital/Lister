import { ActionFunctionArgs, redirect } from "react-router-dom";

export const editListPageAction = async ({ request }: ActionFunctionArgs) => {
  const data = await request.formData();
  const serialized = data.get("serialized") as string;

  const postRequest = new Request(`/api/lists/create`, {
    headers: {
      "Content-Type": "application/json",
    },
    method: "POST",
    body: serialized,
  });
  const response = await fetch(postRequest);
  const json = await response.json();
  const listId = json.id;

  return redirect(`/${listId}`);
};
