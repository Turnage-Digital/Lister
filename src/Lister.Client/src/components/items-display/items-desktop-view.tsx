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
  onDeleteItem,
}: Props) => {
  const gridColDefs = getGridColDefs(definition, onViewItem, onDeleteItem);

  const rows = data.items.map((item) => ({
    id: item.id,
    ...item.bag,
  }));

  return (
    <Paper>
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
      />
    </Paper>
  );
};

export default ItemsDesktopView;
