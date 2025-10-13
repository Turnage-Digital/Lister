import * as React from "react";

import { Box, Grid, Pagination, Stack } from "@mui/material";

import { ListItem, ListItemDefinition } from "../../models";
import ItemCard from "../item-card";

interface Props {
  items: ListItem[];
  definition: ListItemDefinition;
  totalCount: number;
  currentPage: number;
  pageSize: number;
  onPageChange: (page: number) => Promise<void> | void;
  onViewItem?: (listId: string, itemId: number) => Promise<void> | void;
  onEditItem?: (listId: string, itemId: number) => Promise<void> | void;
  onDeleteItem?: (listId: string, itemId: number) => Promise<void> | void;
}

const ItemsMobileView = ({
  items,
  definition,
  totalCount,
  currentPage,
  pageSize,
  onPageChange,
  onViewItem,
  onEditItem,
  onDeleteItem,
}: Props) => {
  const totalPages = Math.ceil(totalCount / pageSize);
  const showPagination = totalPages > 1;
  return (
    <Stack spacing={{ xs: 3, md: 4 }}>
      <Grid container spacing={3}>
        {items.map((item) => (
          <Grid key={item.id} size={{ xs: 12, sm: 6, md: 4 }}>
            <ItemCard
              item={item}
              definition={definition}
              onViewItem={onViewItem}
              onEditItem={onEditItem}
              onDeleteItem={onDeleteItem}
            />
          </Grid>
        ))}
      </Grid>

      {showPagination && (
        <Box sx={{ display: "flex", justifyContent: "center" }}>
          <Pagination
            count={totalPages}
            page={currentPage + 1}
            onChange={async (_event, page) => {
              await onPageChange(page - 1);
              const mainContent = document.querySelector("main");
              if (mainContent) {
                mainContent.scrollTo({ top: 0, behavior: "smooth" });
              }
            }}
            color="primary"
            size="large"
            showFirstButton
            showLastButton
            siblingCount={0}
          />
        </Box>
      )}
    </Stack>
  );
};

export default ItemsMobileView;
