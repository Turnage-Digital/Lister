import { ActionFunctionArgs, redirect } from "react-router-dom";

export const signInPageAction = async ({ request }: ActionFunctionArgs) => {
  const data = await request.formData();

  const email = data.get("username") as string | null;
  if (!email) {
    return {
      error: "You must provide a username to log in",
    };
  }

  const password = data.get("password") as string | null;
  if (!password) {
    return {
      error: "You must provide a password to log in",
    };
  }

  const input = { email, password };
  const postRequest = new Request("/identity/login?useCookies=true", {
    headers: {
      "Content-Type": "application/json",
    },
    method: "POST",
    body: JSON.stringify(input),
  });
  const response = await fetch(postRequest);
  const succeeded = response.status === 200;
  if (!succeeded) {
    return {
      error: "Invalid username or password",
    };
  }

  const redirectTo = data.get("redirectTo") as string | null;
  return redirect(redirectTo || "/");
};
