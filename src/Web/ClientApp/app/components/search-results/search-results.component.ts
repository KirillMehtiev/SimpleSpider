import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: 'search-results',
    templateUrl: './search-results.component.html'
})
export class SearchResultsComponent {
    private websiteUrl: string;

    constructor(route: ActivatedRoute) {
        
    }

}
