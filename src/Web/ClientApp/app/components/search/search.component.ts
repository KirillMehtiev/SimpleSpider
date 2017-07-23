import { Component } from '@angular/core';

@Component({
    selector: 'search',
    templateUrl: './search.component.html'
})
export class SearchComponent {
    public url: string = 'http://www.collective.com';
    public sseUrl: string = 'api/SampleData/Sse?url=';
    public eventSource: EventSource;

    public isWorking: boolean = false;
    public cansellingId: string = undefined;

    find(event: Event) {
        let eventSource = new EventSource(this.sseUrl + this.url);
        this.isWorking = true;

        eventSource.onmessage = function (event) {
            console.log('onmessage: ' + event.data);
            
            if (event.id) {
                this.cansellingId = event.id;
            }
        };

        eventSource.onerror = function (e) {
            eventSource.close()
            this.isWorking = false;
            console.log("EventSource failed.");
        };
    }

    cansel(event: Event) {
        //if (this.isWorking && this.cansellingId !== undefined)
        if (this.eventSource !== undefined)
            this.eventSource.close();
    }

}
