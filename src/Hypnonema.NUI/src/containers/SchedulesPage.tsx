import { FC, useEffect, useState } from "react";
import {
  Button,
  Card,
  CardContent,
  Container,
  Stack,
  Typography,
} from "@mui/material";
import { useNavigate } from "react-router-dom";
import { useAppSelector } from "../hooks/store";
import SimpleBar from "simplebar-react";

import styled from "styled-components";
import { SchedulesTable } from "../components/SchedulesTable";
import { AlertDialog } from "../components/AlertDialog";
import { useNuiRequest } from "fivem-nui-react-lib";
import Schedule from "../types/schedule";

const Wrapper = styled.div`
  margin-top: 30px;
  display: flex;
  justify-content: center;
  flex-direction: column;
  align-items: center;
`;

export const SchedulesPage: FC = () => {
  const [schedulesToDelete, setSchedulesToDelete] = useState(-1);

  const [isAlertOpen, setIsAlertOpen] = useState(false);

  const onCreate = () => {
    navigate("/create-schedule", { replace: true });
  };

  const schedules = useAppSelector((state) => state.schedules.schedules);

  useEffect(() => {
    send("getScheduleList").then(() => {});
  }, []);

  const { send } = useNuiRequest();

  const navigate = useNavigate();

  return (
    <Container>
      <Stack direction="row" sx={{ marginTop: "50px", marginBottom: "10px" }}>
        <Typography variant="h5">Schedules</Typography>
        <Button
          sx={{ marginLeft: "auto", marginRight: "2rem" }}
          variant="contained"
          onClick={onCreate}
        >
          Create
        </Button>
      </Stack>
      <Wrapper>
        <Card
          sx={{
            width: "100%",
            maxWidth: "100%",
            boxShadow: "6",
          }}
        >
          <SimpleBar
            autoHide={false}
            style={{
              maxHeight: "420px",
            }}
          >
            <CardContent>
              <SchedulesTable
                onEdit={(schedule: Schedule) => {
                  navigate(`/edit-schedule/${schedule.id!}`, { replace: true });
                }}
                schedules={schedules}
                onDelete={(schedule: Schedule) => {
                  setIsAlertOpen(true);
                  setSchedulesToDelete(schedule.id!);
                }}
              />
            </CardContent>
          </SimpleBar>
        </Card>
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
            content="Do you really want do delete the Schedule?"
            open={isAlertOpen}
            onCancel={() => {
              setSchedulesToDelete(-1);
              setIsAlertOpen(false);
            }}
            onAccept={() => {
              send("deleteSchedule", { scheduleId: schedulesToDelete }).then(
                () => {}
              );
              setIsAlertOpen(false);
            }}
          />
        </div>
      </Wrapper>
    </Container>
  );
};
