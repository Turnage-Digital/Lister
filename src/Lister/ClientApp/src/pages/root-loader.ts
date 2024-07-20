import { type LoaderFunctionArgs, redirect } from "react-router-dom";

export const rootLoader = async ({ request }: LoaderFunctionArgs) => {
  const postRequest = new Request("/identity/manage/info", {
    method: "GET",
  });
  const response = await fetch(postRequest);
  if (response.status === 401) {
    const params = new URLSearchParams();
    params.set("callbackUrl", new URL(request.url).pathname);
    return redirect(`/sign-in?${params.toString()}`);
  }
  const retval = await response.json();
  return retval;
};
