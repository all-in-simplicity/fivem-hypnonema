import { FC, useState } from "react";

import { Button, Container, Stack, Typography } from "@mui/material";

import { useAppSelector } from "../hooks/store";
import { ScreensList } from "../components/ScreensList";
import styled from "styled-components";
import { AlertDialog } from "../components/AlertDialog";
import { useNuiRequest } from "fivem-nui-react-lib";

import { useNavigate } from "react-router-dom";

const Wrapper = styled.div``;

export const ScreensPage: FC = () => {
  const screens = useAppSelector((state) => state.screens.screens);

  const [screenToDelete, setScreenToDelete] = useState("");

  const [isAlertOpen, setIsAlertOpen] = useState(false);

  const { send } = useNuiRequest();

  const navigate = useNavigate();

  const onEdit = (id: number) => {
    navigate(`/screen/${id}`, { replace: true });
  };

  const onDelete = (screenName: string) => {
    setScreenToDelete(screenName);

    setIsAlertOpen(true);
  };

  const onCreate = () => {
    navigate("/create-screen", { replace: true });
  };

  const onCancel = () => {
    setScreenToDelete("");

    setIsAlertOpen(false);
  };

  return (
    <Wrapper>
      <Container>
        <Stack direction="row" sx={{ marginTop: "50px", marginBottom: "10px" }}>
          <Typography variant="h5">Screens</Typography>
          <Button
            sx={{ marginLeft: "auto", marginRight: "2rem" }}
            variant="contained"
            onClick={onCreate}
          >
            Create
          </Button>
        </Stack>
        <ScreensList screens={screens} onDelete={onDelete} onEdit={onEdit} />
      </Container>
      <div
        style={{
          position: "absolute",
          right: 0,
          bottom: "53px",
          marginRight: "85px",
        }}
      >
        <AlertDialog
          title="Confirmation required"
          content="Do you really want do delete the Screen?"
          open={isAlertOpen}
          onCancel={onCancel}
          onAccept={() => {
            send("deleteScreen", { screenName: screenToDelete }).then(() => {});
            setIsAlertOpen(false);
          }}
        />
      </div>
    </Wrapper>
  );
};
