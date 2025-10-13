import * as React from "react";

import { Paper } from "@mui/material";
import { DataGrid, GridPaginationModel, GridSortModel } from "@mui/x-data-grid";

import { ListItemDefinition, PagedList } from "../../models";
import { getGridColDefs } from "../col-defs";

interface Props {
  data: PagedList;
  definition: ListItemDefinition;
  paginationModel: GridPaginationModel;
  sortModel: GridSortModel;
  onPaginationChange: (model: GridPaginationModel) => Promise<void> | void;
  onSortChange: (model: GridSortModel) => Promise<void> | void;
  onViewItem: (listId: string, itemId: number) => Promise<void> | void;
  onEditItem: (listId: string, itemId: number) => Promise<void> | void;
  onDeleteItem: (listId: string, itemId: number) => Promise<void> | void;
}

const ItemsDesktopView = ({
  data,
  definition,
  paginationModel,
  sortModel,
  onPaginationChange,
  onSortChange,
  onViewItem,
  onEditItem,
  onDeleteItem,
}: Props) => {
  const gridColDefs = getGridColDefs(
    definition,
    onViewItem,
    onEditItem,
    onDeleteItem,
  );

  const rows = data.items.map((item) => ({
    id: item.id,
    ...item.bag,
  }));

  return (
    <Paper
      variant="outlined"
      sx={{
        p: { xs: 1.5, md: 2.5 },
        backgroundColor: "background.paper",
        boxShadow: "none",
      }}
    >
      <DataGrid
        columns={gridColDefs}
        rows={rows}
        getRowId={(row) => row.id}
        rowCount={data.count}
        paginationMode="server"
        pageSizeOptions={[10, 25, 50]}
        onPaginationModelChange={onPaginationChange}
        sortingMode="server"
        onSortModelChange={onSortChange}
        initialState={{
          pagination: {
            paginationModel,
          },
          sorting: {
            sortModel,
          },
        }}
        disableColumnFilter
        disableColumnSelector
        disableRowSelectionOnClick
        sx={{
          backgroundColor: "background.paper",
          border: "none",
          "& .MuiDataGrid-columnHeaders": {
            borderBottomColor: "divider",
          },
          "& .MuiDataGrid-row": {
            transition: "background-color 0.2s ease",
          },
        }}
      />
    </Paper>
  );
};

export default ItemsDesktopView;
