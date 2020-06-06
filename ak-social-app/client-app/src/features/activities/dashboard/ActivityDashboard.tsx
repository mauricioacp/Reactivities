import React, { useContext, useEffect } from "react";
import { Grid } from "semantic-ui-react";
import AcitivityList from "./AcitivityList";
import { observer } from "mobx-react-lite";
import ActivityStore from '../../../app/stores/activityStore';
import LoadingComponents from "../../../app/layout/LoadingComponents";

const ActivityDashboard: React.FC = () => {
  const activityStore = useContext(ActivityStore);

  useEffect(() => {
    activityStore.loadActivities();
  }, [activityStore]);

  if (activityStore.loadingInitial)
    return <LoadingComponents content="Loading Activities..." />;
  return (
    <Grid>
      <Grid.Column width={10}>
        <AcitivityList />
      </Grid.Column>
      <Grid.Column width={6}>
        <h2>Activity filters</h2>
      </Grid.Column>
    </Grid>
  );
};

export default observer(ActivityDashboard);
