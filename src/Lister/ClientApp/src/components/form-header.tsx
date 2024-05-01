import React from "react";
import Grid from "@mui/material/Unstable_Grid2";
import { Breadcrumbs, Link, Typography } from "@mui/material";
import { useLinkClickHandler, useParams } from "react-router-dom";

interface Props {
  header: string;
  currentRoute: string;
  showPreviousRoute?: boolean;
}

const FormHeader = ({ header, currentRoute, showPreviousRoute }: Props) => {
  const params = useParams();
  const previousUrl = params.listId ? `/${params.listId}` : `/`;
  const handleLinkClick = useLinkClickHandler(previousUrl);

  const listsUrl = "/";
  const handleListsClick = useLinkClickHandler(listsUrl);

  return (
    <Grid container spacing={2}>
      <Grid xs={12}>
        <Typography
          color="primary"
          fontWeight="medium"
          variant="h4"
          component="h1"
        >
          {header}
        </Typography>
      </Grid>

      <Grid xs={12}>
        <Breadcrumbs>
          <Link
            underline="hover"
            onClick={handleListsClick}
            sx={{ cursor: "pointer" }}
          >
            Lists
          </Link>

          <Typography color="text.primary">{currentRoute}</Typography>

          {/* <Link */}
          {/*   underline="hover" */}
          {/*   onClick={handleLinkClick} */}
          {/*   sx={{ cursor: "pointer" }} */}
          {/* > */}
          {/*   {previousRoute} */}
          {/* </Link> */}
          {/* {currentRoute.map((route) => ( */}
          {/*   <Typography key={route} color="text.primary"> */}
          {/*     {route} */}
          {/*   </Typography> */}
          {/* ))} */}
        </Breadcrumbs>
      </Grid>
    </Grid>
  );
};

export default FormHeader;
