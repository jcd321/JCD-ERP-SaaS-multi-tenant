import { Component, inject, OnInit } from '@angular/core';

import { TranslatePipe } from '../../../core/i18n';
import { SettingsFacade } from '../../../store/settings/settings.facade';

@Component({
  selector: 'app-settings-list',
  imports: [TranslatePipe],
  templateUrl: './settings-list.component.html',
  styleUrl: './settings-list.component.scss',
})
export class SettingsListComponent implements OnInit {
  private readonly settingsFacade = inject(SettingsFacade);

  readonly settings = this.settingsFacade.settings;
  readonly loading = this.settingsFacade.loading;
  readonly errorMessage = this.settingsFacade.error;

  ngOnInit(): void {
    this.settingsFacade.loadSettings();
  }
}
