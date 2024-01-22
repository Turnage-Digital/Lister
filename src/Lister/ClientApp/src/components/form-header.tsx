import React from "react";
import Grid from "@mui/material/Unstable_Grid2";
import { Breadcrumbs, Link, Typography } from "@mui/material";
import { useLinkClickHandler, useParams } from "react-router-dom";

interface Props {
  currentHeader: string;
  previousHeader: string;
}

const FormHeader = ({ currentHeader, previousHeader }: Props) => {
  const params = useParams();

  const previousUrl = params.listId
    ? `/${params.listId}?page=1&pageSize=10`
    : "/";

  const handleLinkClick = useLinkClickHandler(previousUrl);

  return (
    <Grid container spacing={2}>
      <Grid xs={12}>
        <Typography
          color="primary"
          fontWeight="medium"
          variant="h4"
          component="h1"
          gutterBottom
        >
          {currentHeader}
        </Typography>
      </Grid>
      <Grid xs={12}>
        <Breadcrumbs color="primary">
          <Link
            underline="hover"
            onClick={handleLinkClick}
            sx={{ cursor: "pointer" }}
          >
            {previousHeader}
          </Link>
          <Typography color="text.primary">Create</Typography>
        </Breadcrumbs>
      </Grid>
    </Grid>
  );
};

export default FormHeader;
