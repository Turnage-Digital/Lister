import * as React from "react";

import { AddCircle } from "@mui/icons-material";
import { Box, Button, CircularProgress, Grid, Stack } from "@mui/material";

import { ListItem, ListItemDefinition } from "../../models";
import ItemCard from "../item-card";

interface Props {
  items: ListItem[];
  definition: ListItemDefinition;
  totalCount: number;
  hasMoreItems: boolean;
  isLoadingMore: boolean;
  onLoadMore: () => Promise<void>;
}

const ItemsMobileView = ({
  items,
  definition,
  totalCount,
  hasMoreItems,
  isLoadingMore,
  onLoadMore,
}: Props) => {
  const displayedCount = items.length;
  const remainingCount = totalCount - displayedCount;

  const loadingMoreButtonText = isLoadingMore
    ? "Loading..."
    : `Load More (${remainingCount} remaining)`;
  const loadingMoreButtonStartIcon = isLoadingMore ? (
    <CircularProgress size={20} />
  ) : (
    <AddCircle />
  );
  return (
    <Stack spacing={4}>
      <Grid container spacing={3}>
        {items.map((item) => (
          <Grid key={item.id} size={{ xs: 12, sm: 6, md: 4 }}>
            <ItemCard item={item} definition={definition} />
          </Grid>
        ))}
      </Grid>

      {hasMoreItems && (
        <Box sx={{ display: "flex", justifyContent: "center" }}>
          <Button
            variant="outlined"
            size="large"
            onClick={onLoadMore}
            disabled={isLoadingMore}
            startIcon={loadingMoreButtonStartIcon}
            fullWidth
            sx={{
              minHeight: 48,
              px: 4,
              borderRadius: 3,
              textTransform: "none",
            }}
          >
            {loadingMoreButtonText}
          </Button>
        </Box>
      )}
    </Stack>
  );
};

export default ItemsMobileView;
