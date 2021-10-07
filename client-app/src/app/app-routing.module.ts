import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { TestErrorComponent } from './errors/test-error/test-error.component';
import { HomeComponent } from './home/home.component';
import { ListComponent } from './list/list.component';
import { MemberDetailComponent } from './member/member-detail/member-detail.component';
import { MemberEditComponent } from './member/member-edit/member-edit.component';
import { MemberListComponent } from './member/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { AdminGuard } from './_guard/admin.guard';
import { AuthGuard } from './_guard/auth.guard';
import { PreventUnsavedChangesGuard } from './_guard/prevent-unsaved-changes.guard';
import { MemberDetailedResolver } from './_resolvers/member-detailed.resolver';

const routes: Routes = [
  {path:"", component:HomeComponent},
    {
      path:"",
      runGuardsAndResolvers: 'always',
      canActivate: [AuthGuard],
      children: [
        {path:"members", component:MemberListComponent},
        {path:'members/:username', component:MemberDetailComponent, resolve: {member: MemberDetailedResolver}},
        {path:'member/edit', component:MemberEditComponent, canDeactivate: [PreventUnsavedChangesGuard]},
        {path:'lists', component:ListComponent},
        {path:"messages", component:MessagesComponent},
        {path:"admin", component:AdminPanelComponent, canActivate: [AdminGuard]},
      ]
    },
    {path:"error", component:TestErrorComponent},
    {path:"not-found", component:NotFoundComponent},
    {path:"server-error", component:ServerErrorComponent},
  {path:"**", component:HomeComponent, pathMatch:"full"}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
