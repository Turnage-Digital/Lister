import React, { ReactElement } from "react";
import { Box } from "@mui/material";

import { ContentSection, TopSection } from "./layout";
import { Loading } from "./components";
import { useAuth, SignInForm } from "./auth";

const Shell = () => {
  const { signedIn, loading, error, signIn } = useAuth();

  let content: ReactElement;

  if (signedIn) {
    content = (
      <Box sx={{ display: "flex" }}>
        <TopSection />
        <ContentSection />
      </Box>
    );
  } else if (loading) {
    content = <Loading />;
  } else {
    content = <SignInForm signIn={signIn} error={error} />;
  }

  return <>{content}</>;
};

export default Shell;
