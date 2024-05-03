import React from "react";
import { Container, Stack, Typography } from "@mui/material";
import Grid from "@mui/material/Unstable_Grid2";

// export const listItemIdPageLoader = async ({ params }: LoaderFunctionArgs) => {
//   if (!params.listId || !params.itemId) {
//     return null;
//   }
//
//   const [getListItemDefinitionResponse, getItemResponse] = await Promise.all([
//     fetch(
//       `${process.env.PUBLIC_URL}/api/lists/${params.listId}/itemDefinition`
//     ),
//     fetch(`/api/lists/${params.listId}/items/${params.itemId}`),
//   ]);
//
//   const listItemDefinition = await getListItemDefinitionResponse.json();
//   const item = await getItemResponse.json();
//
//   return { listItemDefinition, item };
// };

const ListItemIdPage = () => {
  // const { listItemDefinition, item } = useLoaderData() as {
  //   listItemDefinition: ListItemDefinition;
  //   item: Item;
  // };

  return (
    <Container maxWidth="xl">
      <Stack sx={{ px: 2, py: 4 }}>
        <Grid container sx={{ py: 4 }}>
          <Grid xs={12} md={9}>
            <Typography
              color="primary"
              fontWeight="medium"
              variant="h4"
              component="h1"
            >
              {/* {`Item #${item.id} - ${listItemDefinition.name}`} */}
            </Typography>
          </Grid>

          <Grid xs={12} md={3} display="flex" justifyContent="flex-end" />
        </Grid>
      </Stack>
    </Container>
  );
};

export default ListItemIdPage;
