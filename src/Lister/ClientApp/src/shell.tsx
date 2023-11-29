import React from "react";
import { Box } from "@mui/material";
import { LoaderFunctionArgs, redirect } from "react-router-dom";

import { ContentSection, TopSection } from "./layout";

export const shellLoader = async ({ request }: LoaderFunctionArgs) => {
  const postRequest = new Request(
    `${process.env.PUBLIC_URL}/api/users/claims`,
    {
      method: "GET",
    }
  );
  const response = await fetch(postRequest);
  if (response.status === 401) {
    const params = new URLSearchParams();
    params.set("callbackUrl", new URL(request.url).pathname);
    return redirect(`/sign-in?${params.toString()}`);
  }
  const retval = await response.json();
  return retval;
};

const Shell = () => {
  return (
    <Box sx={{ display: "flex" }}>
      <TopSection />
      <ContentSection />
    </Box>
  );
};

export default Shell;
